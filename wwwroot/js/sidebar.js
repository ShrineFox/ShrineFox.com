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