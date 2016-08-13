<?
$root = "/"
$folder = "tmp";
$mystring = system('python /usr/local/python/kana/text-to-obj.py ハニョ /usr/local/python/kana', $retval);
$file = "/usr/local/python/kana/ハニョ.obj";

if(!$file){ // file does not exist
    die('file not found');
} else {
    header("Cache-Control: public");
    header("Content-Description: File Transfer");
    header("Content-Disposition: attachment; filename=$file");
    header("Content-Type: application/zip");
    header("Content-Transfer-Encoding: binary");

    // read the file from disk
    readfile($file);
}
?>