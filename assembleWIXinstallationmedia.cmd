mkdir WIX_MFP_Build
mkdir WIX_MFP_Build\runtime
mklink /j "WIX_MFP_Build\eclipse" "eclipse"
mklink /j "WIX_MFP_Build\swidtag" "swidtag"
mklink /j "WIX_MFP_Build\readmes" "readmes"
mklink /j "WIX_MFP_Build\license" "license\rtf"
mklink /j "WIX_MFP_Build\runtime\amd64_nt_4" "runtime\amd64_nt_4"
mklink /j "WIX_MFP_Build\runtime\amd64_nt_4\setup" "runtime\setup"