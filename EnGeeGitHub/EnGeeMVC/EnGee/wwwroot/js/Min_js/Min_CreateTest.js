$(document).ready(function () {
    // 隐藏所有步骤，除了第一个
    $("#msform fieldset").not(':first').hide();

    // 当点击“Next”按钮时
    $(".next").click(function () {
        var current_fs = $(this).parent();
        var next_fs = $(this).parent().next();

        current_fs.hide();
        next_fs.show();
    });

    // 当点击“Previous”按钮时
    $(".previous").click(function () {
        var current_fs = $(this).parent();
        var previous_fs = $(this).parent().prev();

        current_fs.hide();
        previous_fs.show();
    });
});
