using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Security.Permissions;
using System.Security.Principal;

namespace CreateMFPToolkitLinks
{
    class Program
    {
        static void Main(string[] args)
        {
            Boolean bDeleteLinks = false;
            string MFP_path = string.Empty;
			// TBD - Pass this from Bundle.wxs
            string version = "3.0.2.0";
            Boolean isUpgrade = false;

            logToFile("*-*-*-*-*-*-*  CreateMFPToolkitLinks v3.0.1.3  *-*-*-*-*-*-*");
            logToFile("Start time:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            /*
            int count = 0;
            foreach (string arg in args)
            {
                count += 1;
                Console.WriteLine("arg{0} is: {1}",
                    count.ToString(), arg);
            }
            */
            if ( args.GetLength(0) < 2)
            {
                logToFile("Not enough args, =" + args.GetLength(0).ToString());
                Environment.Exit( -1 );
            }
             
            if ( args[0].Equals( "-remove" ) )
            {
                logToFile("Removing toolkit links from:" + args[1]);
                bDeleteLinks = true;
                MFP_path = args[1];
            }
            else if ( args[0].Equals( "-create" ) )
            {
                logToFile("Creating toolkit links to: " + args[1]);
                bDeleteLinks = false;
                MFP_path = args[1];
            }
            else
            {
                logToFile("Invalid first argument: "+args[0]);
                Environment.Exit(-1);
            }
                       
			try
            {
				//string version = args[2];
				logToFile("Version of AAA: " + version);		   
				isUpgrade = checkForUpgrade(version);
			}
			catch (Exception)
            {
                logToFile("Failure whiling checking for upgrade");
            } 
                         
            WindowsPrincipal myPrincipal = new WindowsPrincipal (WindowsIdentity .GetCurrent());
            if (myPrincipal.IsInRole(WindowsBuiltInRole .Administrator) == false )
            {
                logToFile("Must be run as administrator/elevated");
                Environment.Exit(-1);
            }


            // Open the 64bit ABC key.
            RegistryKey localKey =
                RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine,
                    RegistryView.Registry64); 
            logToFile("ABC_key : "+localKey);        
                    
                        
            try
            {
                RegistryKey ABC_key = localKey.OpenSubKey("SOFTWARE").OpenSubKey("Dummy").OpenSubKey("Dummy Integration Bus");
                  // How many version are installed.
	            logToFile("There is/are " + ABC_key.SubKeyCount.ToString() + " version/s of ABC v10 (or higher) installed.");
	            logToFile("ABC_key : "+ABC_key);
	            logToFile("Under the keyname: "+ABC_key.Name);
	            
	                   
		            foreach (string subKeyName in ABC_key.GetSubKeyNames())
		            {
		                using (RegistryKey
		                    perVerKey = ABC_key.OpenSubKey(subKeyName))
		                {
		                    string ABC_path = perVerKey.GetValue("install_location").ToString();
		                    logToFile(subKeyName + " is installed at: " + ABC_path);
		
		                    if (bDeleteLinks)
		                        DeleteToolkitLink(ABC_path, isUpgrade);
		                    else
		                        CreateToolkitLink(MFP_path, ABC_path);
		
		                }
		            }
	            
            }
            catch (Exception)
            {
                logToFile("Failed to fetch key - Dummy Integration Bus");
            }  
                
            try
            {    
	                
	            RegistryKey XYZ_key =
	                localKey.OpenSubKey("SOFTWARE").OpenSubKey("Dummy").OpenSubKey("Dummy XYZ");        
	            logToFile("There is/are " + XYZ_key.SubKeyCount.ToString() + " version/s of XYZ v11 (or higher) installed.");
	            logToFile("XYZ_key : "+XYZ_key);
	            logToFile("Under the keyname: "+XYZ_key.Name);  
            
            	
		            foreach (string subKeyName in XYZ_key.GetSubKeyNames())
		            {
		                using (RegistryKey
		                    perVerKey = XYZ_key.OpenSubKey(subKeyName))
		                {
		                    string XYZ_path = perVerKey.GetValue("install_location").ToString();
		                    logToFile(subKeyName + " is installed at: " + XYZ_path);
		
		                    if (bDeleteLinks)
		                        DeleteToolkitLink(XYZ_path, isUpgrade);
		                    else
		                        CreateToolkitLink(MFP_path, XYZ_path);
		
		                }
		            }
	                  
            }
            catch (Exception)
            {
                logToFile("Failed to fetch key - Dummy XYZ");
            }  
           
            logToFile(Environment.NewLine + "Finished creating Toolkit link files.");
            logToFile(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+Environment.NewLine);
        }
        
        private static Boolean checkForUpgrade(string currentVer) 
        {
        	Boolean isUpgrade = false;
        	currentVer = currentVer.Replace(".", string.Empty);
        	
        	logToFile("Version after removing dots is: " + currentVer);
        	
        	RegistryKey localKey =
                RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine,
                    RegistryView.Registry64);
        	
			RegistryKey AAA_key =
	                localKey.OpenSubKey("SOFTWARE").OpenSubKey("Dummy").OpenSubKey("Dummy AAA");	
	                
	        foreach (string subKeyName in AAA_key.GetSubKeyNames())
	        {
	        	logToFile("AAA subKeyName: " + subKeyName);
	        	string versionKey = subKeyName.Replace(".", string.Empty);
	        	
	        	if(Int32.Parse(versionKey) > Int32.Parse(currentVer)) {
	        		isUpgrade = true;
	        	}
	        }
	        	                
        	return isUpgrade;
        }

        private static void CreateToolkitLink(string MFP_path, string ABC_path)
        {
            string linkFolder = ABC_path + @"tools\links";
            string linkPath = ABC_path + @"tools\links\dummy.v2.link";

            // Create dir
            if (!Directory.Exists(linkFolder))
            {
                logToFile("Creating Folder: " + linkFolder);
                Directory.CreateDirectory(linkFolder);
            }
            // Delete existing file if there
            if (File.Exists(linkPath))
            {
                logToFile("Deleting existing link file: " + linkPath);
                try
                {
                    File.Delete(linkPath);
                }
                catch (Exception)
                {
                    logToFile("Failed to delete link file: " + linkPath);
                }
            }
            // Create link file
            // Open the stream and write to it. 
            using (FileStream fs = File.OpenWrite(linkPath))
            {
                // Fix path so as to make it unix style backslashes.
                // yes this is redictulus that a java program running on Windows
                // can't cope with a windows style path - that's eclipse ... for you.
                string unixPath = MFP_path;
                unixPath = unixPath.Replace(@"\", @"/");

                // now strip the the '/' off - if there is a trailing one
                int lastSlash = unixPath.LastIndexOf('/');
                unixPath = (lastSlash >= unixPath.Length-1) ? unixPath.Substring(0, unixPath.Length - 1) : unixPath;

                Byte[] info = new UTF8Encoding(true).GetBytes("path=" + unixPath);

                // Write MFP install path into the file.
                fs.Write(info, 0, info.Length);
            }
            logToFile("Created link file: " + linkPath);
        }

        private static void DeleteToolkitLink(string ABC_path, Boolean isUpgrade)
        {
        	if(!isUpgrade) {
        		string linkFolder = ABC_path + @"tools\links";
	            string linkPath = ABC_path + @"tools\links\dummy.v2.link";
	
	            // Delete existing file if there
	            try
	            {
	            	if (File.Exists(linkPath))
		            {
		                logToFile("Deleting existing link file: " + linkPath);
		                File.Delete(linkPath);
		            }
	            }
                catch (Exception)
                {
                    logToFile("Failed to delete link file: " + linkPath);
                }
	            
	            
	            // Delete folder if empty
	            try 
	            {
	            	if (Directory.EnumerateFileSystemEntries(linkFolder).ToArray().GetLength(0) == 0)
		            {
		                logToFile("Deleting links Folder: " + linkFolder);
		                Directory.Delete(linkFolder);
		            }
	            }
	            catch (Exception)
                {
                    logToFile("Failed to delete Folder: " + linkFolder);
                }
	           
        	}
        }

        private static void logToFile(string logString)
        {
            // Create log file
            string logPath=System.Environment.GetEnvironmentVariable ("temp")+@"\ABCMFPCreateToolkitLink.log";
            using (FileStream logfs = new FileStream(logPath, FileMode.Append, FileAccess.Write))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(logString+Environment.NewLine);

                // Write log info to file.
                logfs.Write(info, 0, info.Length);
            }
        }
    }
}
