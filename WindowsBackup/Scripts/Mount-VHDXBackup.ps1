Param(

    [string] $BackupFolder

)

$BackupFolder = "D:\WindowsBackupManager\25092020"

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

Mount-VHD -Path $VHDBackup.FullName

$Disk = Get-Disk | Where-Object -Property Location -EQ $VHDBackup.FullName
$Disk

Write-Host 'Suche Partition'
Get-Partition | Where-Object -Property DiskID -EQ $Disk.Path | Sort-Object -Property Size -Descending | Select-Object -First 1 | Set-Partition -NewDriveLetter $DriveLetter

Write-Host 'Drücke Enter um Wiederherstellung zu beenden.' -ForegroundColor Green
pause


Dismount-VHD -Path $VHDBackup.FullName