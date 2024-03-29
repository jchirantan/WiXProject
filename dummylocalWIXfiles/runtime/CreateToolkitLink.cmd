::create folders
if not exist "%~1\tools\links" md "%~1\tools\links"
::create .link file
set path=%~2
echo path=%path:\=/% > "%~1\tools\links\dummy.v2.link"