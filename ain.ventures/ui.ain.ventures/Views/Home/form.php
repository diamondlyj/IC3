$term = mysql_real_escape_string($_REQUEST['term']);    
$sql = "GetQPoint_ByCategoryLike ".$term"";
$r_query = mysql_query($sql);
