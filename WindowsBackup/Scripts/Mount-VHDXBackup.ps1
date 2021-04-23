Param(

    [string] $BackupFolder

)


$Letters = 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'

foreach($Letter in $Letters) 
{

        $Drive = Get-PSDrive $Letter

        if($Drive -eq $null) 
        {
            $DriveLetter = $Letter
            Write-Host "Laufwerksbuchstaben gefunden: $DriveLetter"
            break;
        }
}

$VHDBackup = Get-ChildItem -Path "$BackupFolder\WindowsImageBackup\" *.vhdx -Recurse | Sort-Object -Property Length -Descending | Select-Object -First 1

$VHDBackup

$Partition = Mount-DiskImage -ImagePath $VHDBackup.FullName -PassThru | Get-Disk | Get-Partition 
$Partition | Where-Object Type -eq Basic | Set-Partition -NewDriveLetter $DriveLetter
Dismount-VHD -Path $VHDBackup.FullName
Mount-VHD $VHDBackup.FullName

Write-Host 'Drücke Enter um Wiederherstellung zu beenden.' -ForegroundColor Green
pause


Dismount-VHD -Path $VHDBackup.FullName





