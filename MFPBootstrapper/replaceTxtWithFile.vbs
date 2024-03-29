'This script has overloaded function to restore the original file
If WScript.Arguments.Count = 1 then
  RestoreOrig WScript.Arguments.Item(0)
  WScript.Quit
end If

If WScript.Arguments.Count <> 3 then
  WScript.Echo "usage: replaceTxtWithFile.vbs target_filename txt_to_find replace_with_filename"
  WScript.Quit
end If

BackUpOrig WScript.Arguments.Item(0)
FindAndReplace WScript.Arguments.Item(0) & ".orig", WScript.Arguments.Item(0), WScript.Arguments.Item(1), WScript.Arguments.Item(2) 

function FindAndReplace(strFilename, strOutputFilename, strFind, strReplaceFilename)
    Set inputFile = CreateObject("Scripting.FileSystemObject").OpenTextFile(strFilename, 1, false, -1)
    strInputFile = inputFile.ReadAll
    inputFile.Close
    Set inputFile = Nothing

    Set replaceFile = CreateObject("Scripting.FileSystemObject").OpenTextFile(strReplaceFilename, 1, false, -1)
    strReplaceFile = replaceFile.ReadAll
    replaceFile.Close
    Set replaceFile = Nothing

    Set outputFile = CreateObject("Scripting.FileSystemObject").OpenTextFile(strOutputFilename, 2, true, -1)
    outputFile.Write( Replace(strInputFile, strFind, strReplaceFile) )
    outputFile.Close
    Set outputFile = Nothing
end function 

function BackUpOrig(strFilename)
    Set backOrig = CreateObject("Scripting.FileSystemObject")
    If (backOrig.FileExists(strFilename & ".orig")) Then 
      backOrig.DeleteFile strFilename & ".orig"
    End If
    backOrig.MoveFile strFilename, strFilename & ".orig"
    Set backOrig = Nothing
end function 

function RestoreOrig(strFilename)
    Set origFile = CreateObject("Scripting.FileSystemObject")
    If (origFile.FileExists(strFilename & ".orig")) Then 
      If (origFile.FileExists(strFilename)) Then 
        origFile.DeleteFile strFilename
      End If 
      origFile.MoveFile strFilename & ".orig", strFilename 
    End If 
    Set origFile = Nothing
end function 