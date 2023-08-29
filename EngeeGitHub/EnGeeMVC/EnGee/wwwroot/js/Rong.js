document.getElementById("CollectEndDate").min = new Date().toISOString().split("T")[0];


// 上傳圖片 圖片預覽
$('#CollectImagePath').on('change', function (e) {
    const file = this.files[0];
    const objectURL = URL.createObjectURL(file);    // 使用 createObjectURL 產生圖片url
    $('#photo').attr('src', objectURL);

});