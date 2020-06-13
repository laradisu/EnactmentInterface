import os
import subprocess
import re
import socket
import json
import struct
import shlex
import sys

os.chdir('/home/elx-lab/darknet/build/darknet/x64')
process = subprocess.Popen(shlex.split('./../../../darknet detector demo data/objFourCombo.data yolo-obj-fourCombo.cfg backupFourCombo/yolo-obj-fourCombo_8000.weights -c 0 -ext_output'), stdout=subprocess.PIPE)

# sample terminal output
# Objects:
#
# elxlablogomarker: 100%  (left_x:  497   top_y:  257   width:   91   height:   96)
# elxlablogomarker: 54%   (left_x:  308   top_y:  374   width:   49   height:   82)
# [2J[1;1H
# FPS:23.8

ip_address = "127.0.0.1"
udpPorts = [6000, 6001, 6002, 6003]
namesTracking = ['neutral', 'boy_aa', 'girl_w', 'cat']
priorConfidences = [-1, -1, -1, -1]
bestXs = [-1, -1, -1, -1]
bestYs = [-1, -1, -1, -1]
bestWidths = [-1, -1, -1, -1]
bestHeights = [-1, -1, -1, -1]
shouldSend = False

while True:
    toutput = process.stdout.readline()
    if toutput is None:
        continue
    output = str(toutput)
    print(output)

    objectText = re.search('Objects:', output)
    if objectText is not None:
        shouldSend = True
        continue

    confidences = [-1, -1, -1, -1]
    for index, name in enumerate(namesTracking):
        confidences[index] = re.search(f'{name}:[ ]+\d+[%]+', output)

    allNone = True
    for name in namesTracking:
        if name is not None:
            allNone = False
            break
    if allNone:
        continue

    for index, confidence in enumerate(confidences):
        if confidence is not None:  # it found the confidence, so the whole line (with x,y,name,etc) exists
            confidence = re.search('\d+', confidence[0])

            if int(confidence[0]) >= priorConfidences[index]:
                priorConfidences[index] = int(confidence[0])

                x = re.search('left_x:[ ]+\d+', output)
                if x is not None:
                    x = re.search('\d+', x[0])
                    bestXs[index] = float(x[0])
                    #print('x is ' + x[0])

                y = re.search('top_y:[ ]+\d+', output)
                if y is not None:
                    y = re.search('\d+', y[0])
                    bestYs[index] = float(y[0])
                    #print('y is ' + y[0])

                width = re.search('width:[ ]+\d+', output)
                if width is not None:
                    width = re.search('\d+', width[0])
                    bestWidths[index] = float(width[0])
                    #print('width is ' + width[0])

                height = re.search('height:[ ]+\d+', output)
                if height is not None:
                    height = re.search('\d+', height[0])
                    bestHeights[index] = float(height[0])
                    #print('height is ' + height[0])

    if shouldSend:
        atLeastOne = False
        for index, priorConfidence in enumerate(priorConfidences):
            if priorConfidence >= 0:
                byteData = struct.pack("<dddd", bestXs[index], bestYs[index], bestWidths[index], bestHeights[index])
                sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
                sock.sendto(byteData, (ip_address, udpPorts[index]))
                bestXs[index] = str(bestXs[index])
                bestYs[index] = str(bestYs[index])
                bestWidths[index] = str(bestWidths[index])
                bestHeights[index] = str(bestHeights[index])
                print(
                    f'Best: confidence={priorConfidence}, x={bestXs[index]}, y={bestYs[index]}, width={bestWidths[index]}, height={bestHeights[index]}')

                bestXs[index] = -1
                bestYs[index] = -1
                bestWidths[index] = -1
                bestHeights[index] = -1
                priorConfidence = -1
                atLeastOne = True

        if atLeastOne:
            shouldSend = False
        else:
            doNotReset = False
            for priorConfidence in priorConfidences:
                if priorConfidence > 0:
                    doNotReset = True

            if not doNotReset:
                bestXs = [-1, -1, -1, -1]
                bestYs = [-1, -1, -1, -1]
                bestWidths = [-1, -1, -1, -1]
                bestHeights = [-1, -1, -1, -1]
                priorConfidences = [-1, -1, -1, -1]

                shouldSend = False
