$names = Get-content "C:\Users\user446\Desktop\OvenDownNotify\BatFilePing\PingCheck\IP.txt"
$date = Get-date
foreach ($name in $names){
  if (Test-Connection -ComputerName $name -Count 1 -ErrorAction SilentlyContinue){
   $Output+= "$name : Online`r`n"
   Write-Host "$name : Online"
  }
  else{
    $Output+= "$name : Offline`r`n"
    Write-Host "$name : Offline"
  }
}
$Output,$date | Out-file "C:\Users\user446\Desktop\OvenDownNotify\BatFilePing\PingCheck\Result.txt"