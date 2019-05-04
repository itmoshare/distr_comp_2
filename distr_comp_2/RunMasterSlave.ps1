Start-Process -FilePath 'dotnet' -ArgumentList './Master/bin/Debug/netcoreapp2.1/Master.dll ./file1.txt ./file2.txt ./file3.txt'
Start-Process -FilePath 'dotnet' -ArgumentList './Worker/bin/Debug/netcoreapp2.1/Worker.dll 0'
Start-Process -FilePath 'dotnet' -ArgumentList './Worker/bin/Debug/netcoreapp2.1/Worker.dll 1'