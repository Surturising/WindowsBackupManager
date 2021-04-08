Param(

    [string] $BackupFolder

)



$Backup = Get-Item -Path "$BackupFolder"


Move-Item -Path $Backup.FullName -Destination $Backup.Root

Write-Host 'Zum Beenden Eingabetaste drücken'

pause