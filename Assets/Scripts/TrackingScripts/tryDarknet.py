import os
import subprocess
import re
import socket
import json
import struct
import shlex
import sys

#os.chdir('C:\\Users\\u2012\\Downloads\\NJRPROJECTBACKUP\\darknet-master\\build\\darknet\\x64')
os.chdir('/home/elx-lab/darknet/build/darknet/x64')
#process = subprocess.Popen(['./../../../darknet','detector','demo','data/objFourCombo.data','yolo-obj-fourCombo.cfg','backupFourCombo/yolo-obj-fourCombo_8000.weights','-c','0','-ext_output'], stdout=subprocess.PIPE, creationflags=subprocess.CREATE_NEW_CONSOLE)
process = subprocess.Popen(shlex.split('./../../../darknet detector demo data/objFourCombo.data yolo-obj-fourCombo.cfg backupFourCombo/yolo-obj-fourCombo_8000.weights -c 0 -ext_output'), stdout=subprocess.PIPE)

# sample terminal output
# Objects:
#
# elxlablogomarker: 100%  (left_x:  497   top_y:  257   width:   91   height:   96)
# elxlablogomarker: 54%   (left_x:  308   top_y:  374   width:   49   height:   82)
# [2J[1;1H
# FPS:23.8

ip_address = "127.0.0.1"
ip_addressR = "127.0.0.2"
port = 9000
portR = 6000
shouldSend = False
namesTracking = ['neutral', 'boy_aa', 'girl_w', 'cat']
priorConfidences = [-1, -1, -1, -1]
bestX = -1
bestXR = -1
bestY = -1
bestYR = -1
bestWidth = -1
bestWidthR = -1
bestHeight = -1
bestHeightR = -1

while True:
    toutput = process.stdout.readline()
    output = str(toutput)
    print(output)

    objectText = re.search('Objects:', output)
    if objectText is not None:
        shouldSend = True

    labelText = re.search(f'{namesTracking}:', output)
    labelTextR = re.search(f'{namesTrackingR}:', output)
    
    
    allNone = true
    for name in namesTracking:
        if (name is not None):
            allNone = false
            break
    if allNone == true
        continue

    confidence = re.search(f'{namesTracking}:[ ]+\d+[%]+', output)
    confidenceR = re.search(f'{namesTrackingR}:[ ]+\d+[%]+', output)
    
    if confidence is not None:
        confidence = re.search('\d+', confidence[0])

        if int(confidence[0]) < priorConfidence:
            pass
        else:
            priorConfidence = int(confidence[0])
            
            x = re.search('left_x:[ ]+\d+', output)
            if x is not None:
                x = re.search('\d+', x[0])
                bestX = float(x[0])
                #print('x is ' + x[0])
    
            y = re.search('top_y:[ ]+\d+', output)
            if y is not None:
                y = re.search('\d+', y[0])
                bestY = float(y[0])
                #print('y is ' + y[0])
    
            width = re.search('width:[ ]+\d+', output)
            if width is not None:
                width = re.search('\d+', width[0])
                bestWidth = float(width[0])
                #print('width is ' + width[0])
    
            height = re.search('height:[ ]+\d+', output)
            if height is not None:
                height = re.search('\d+', height[0])
                bestHeight = float(height[0])
                #print('height is ' + height[0])
    
    if confidenceR is not None:
        confidenceR = re.search('\d+', confidenceR[0])
        
        if int(confidenceR[0]) < priorConfidenceR:
            pass
        else:
            priorConfidenceR = int(confidenceR[0])
    
            xR = re.search('left_x:[ ]+\d+', output)
            if xR is not None:
                xR = re.search('\d+', xR[0])
                bestXR = float(xR[0])
                #print('x is ' + x[0])
            
            yR = re.search('top_y:[ ]+\d+', output)
            if yR is not None:
                yR = re.search('\d+', yR[0])
                bestYR = float(yR[0])
                #print('y is ' + y[0])
            
            widthR = re.search('width:[ ]+\d+', output)
            if widthR is not None:
                widthR = re.search('\d+', widthR[0])
                bestWidthR = float(widthR[0])
                #print('width is ' + width[0])
            
            heightR = re.search('height:[ ]+\d+', output)
            if heightR is not None:
                heightR = re.search('\d+', heightR[0])
                bestHeightR = float(heightR[0])
                #print('height is ' + height[0])

    if (shouldSend and ((priorConfidence >= 0) or (priorConfidenceR >= 0))):
        #data = '' + int(x[0]) + ',' + y[0] + ',' + width[0] + ',' + height[0]
        if (priorConfidence >= 0):
            byteData = struct.pack("<dddd", bestX, bestY, bestWidth, bestHeight)
            sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            sock.sendto(byteData, (ip_address, port))
            bestX = str(bestX)
            bestY = str(bestY)
            bestWidth = str(bestWidth)
            bestHeight= str(bestHeight)
            print(f'Best: confidence={priorConfidence}, x={bestX}, y={bestY}, width={bestWidth}, height={bestHeight}')

            bestX = -1
            bestY = -1
            bestWidth = -1
            bestHeight = -1
            priorConfidence = -1
        if (priorConfidenceR >= 0):
            byteDataR = struct.pack("<dddd", bestXR, bestYR, bestWidthR, bestHeightR)
            sockR = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            sockR.sendto(byteDataR, (ip_addressR, portR))
            bestXR = str(bestXR)
            bestYR = str(bestYR)
            bestWidthR = str(bestWidthR)
            bestHeightR= str(bestHeightR)
            print(f'BestR: confidenceR={priorConfidenceR}, xR={bestXR}, yR={bestYR}, widthR={bestWidthR}, heightR={bestHeightR}')

            bestXR = -1
            bestYR = -1
            bestWidthR = -1
            bestHeightR = -1
            priorConfidenceR = -1
        
        shouldSend = False

    elif (shouldSend and ((priorConfidence < 0) and (priorConfidenceR < 0))):
        bestX = -1
        bestY = -1
        bestWidth = -1
        bestHeight = -1
        priorConfidence = -1
        
        bestXR = -1
        bestYR = -1
        bestWidthR = -1
        bestHeightR = -1
        priorConfidenceR = -1
        
        shouldSend = False

