$(document).ready(function () {
    $('.menu-button').click(function () {
        $('#sidebar').addClass('show');
        $('#maincontent').addClass('show');
        setSidebarState(true);
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
        setSidebarState(false);
    });

    restoreSidebarState();
    setTimeout(restoreSidebarState, 0);
});

window.initUI = function () {
    // Menu open
    $(document)
        .off("click.ui", ".menu-button")
        .on("click.ui", ".menu-button", function () {
            $("#sidebar").addClass("show");
            $("#maincontent").addClass("show");
            setSidebarState(true);
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
            setSidebarState(false);
        });

    restoreSidebarState();
    setTimeout(restoreSidebarState, 0);
};

function restoreSidebarState() {
    const sidebarState = getSidebarCookie('sidebar_state');
    const isDesktop = window.innerWidth >= 1200;
    const shouldOpenByDefault = !sidebarState && isDesktop;
    if (!isDesktop) return;
    if (sidebarState !== 'open' && !shouldOpenByDefault) return;

    $('#sidebar').addClass('show');
    $('#maincontent').addClass('show');
}

function setSidebarState(isOpen) {
    const expiration = new Date();
    expiration.setDate(expiration.getDate() + 999);
    document.cookie = `sidebar_state=${isOpen ? 'open' : 'closed'}; expires=${expiration.toUTCString()}; max-age=86313600; path=/`;
}

function getSidebarCookie(name) {
    const prefix = `${name}=`;
    const cookie = document.cookie.split(';').map(value => value.trim())
        .find(value => value.startsWith(prefix));

    return cookie ? decodeURIComponent(cookie.substring(prefix.length)) : '';
}