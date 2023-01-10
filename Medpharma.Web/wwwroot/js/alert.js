window.setTimeout(function () {
    $(".alert.notification").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 2000);