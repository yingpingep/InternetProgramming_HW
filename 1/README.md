# Internet Programming homework 1
# Require
* .NET Core 1.0 (Or newer)  
You can get CLI tools from [HERE](https://www.microsoft.com/net/core)
* [Version 1.0 OS Support :](https://github.com/dotnet/core/blob/master/roadmap.md)

OS|Version|Architectures|Configurations|Notes
------------------------------|-------------------------------|----------|----------|---------|
Windows Client                | 7 SP1 - 10                    | x64, x86 | |
Windows Server                | 2008 R2 SP1 - 2016            | x64, x86 | Full, Server Core, Nano (2016 only) |
Red Hat Enterprise Linux      | 7.2                           | x64      | |
Fedora                        | 23                            | x64      | |
Debian                        | 8.2                           | x64      | |
Ubuntu                        | 14.04 LTS, 16.04 LTS          | x64      | |
Linux Mint                    | 17                            | x64      | |
openSUSE                      | 13.2                          | x64      | |
Centos                        | 7.1                           | x64      | |
Oracle Linux                  | 7.1                           | x64      | |
Mac OS X                       | 10.11, 10.12            | x64      | | 10.12 added in 1.0.2

# How it work
1. Install .NET Core on your computer

2. Make sure your files (which you wanna transfer) are in the Server folder

3. Server Side (Must in "Server" folder)   
    dotnet run \<IP Address\> \<Port Number\>

4. Client Side (Must in "Client" folder)  
    dotnet run \<Server IP address\> \<Server Port\> \<File name\>

5. Your file (from server) will be created with same name on server in your Client folder