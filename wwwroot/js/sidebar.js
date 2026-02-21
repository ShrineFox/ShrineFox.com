$(document).ready(function () {
    $('.menu-button').click(function () {
        $('#sidebar').addClass('show');
        $('#maincontent').addClass('show');
    });

    $('#theme-toggle').click(function () {
        $('#theme-modal').addClass('active');
    });

    $('#theme-modal-close').click(function () {
        $('#theme-modal').removeClass('active');
    });

    $('.closebtn').click(function () {
        $('#sidebar').removeClass('show');
        $('#maincontent').removeClass('show');
    });
});

window.initUI = function () {

    // Menu open
    $(document)
        .off("click.ui", ".menu-button")
        .on("click.ui", ".menu-button", function () {
            $("#sidebar").addClass("show");
            $("#maincontent").addClass("show");
        });

    // Theme modal open
    $(document)
        .off("click.ui", "#theme-toggle")
        .on("click.ui", "#theme-toggle", function () {
            $("#theme-modal").addClass("active");
        });

    // Theme modal close
    $(document)
        .off("click.ui", "#theme-modal-close")
        .on("click.ui", "#theme-modal-close", function () {
            $("#theme-modal").removeClass("active");
        });

    // Sidebar close
    $(document)
        .off("click.ui", ".closebtn")
        .on("click.ui", ".closebtn", function () {
            $("#sidebar").removeClass("show");
            $("#maincontent").removeClass("show");
        });
};