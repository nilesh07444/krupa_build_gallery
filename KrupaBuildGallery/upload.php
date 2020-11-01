<?php
$FolderName = $_GET['FolderName'];
$Flname = $_GET['Flname'];
$uploads_dir = 'Images/' . $FolderName;
if (!is_dir($uploads_dir)) {
   mkdir($uploads_dir);
}
//$uploads_dir = 'uploadfiles'; //Directory to save the file that comes from client application.
if ($_FILES["file"]["error"] == UPLOAD_ERR_OK) {
   $tmp_name = $_FILES["file"]["tmp_name"];
   $name = $_FILES["file"]["name"];
   $exts = get_file_extension($name);
   $FlineNm = $Flname.".".$exts;
   $Result = move_uploaded_file($tmp_name, "$uploads_dir/$FlineNm");

//    echo "Success: Picture Upload Successfully!";
   echo $FlineNm;
}

function get_file_extension($file_name) {
   return substr(strrchr($file_name, '.'), 1);
}

?>


