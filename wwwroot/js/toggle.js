jQuery(document).ready(function () {
    // If any toggle-title has "active", show its corresponding .toggle-inner
    jQuery(".toggle .toggle-title.active").closest('.toggle').find('.toggle-inner').show();

    // Click event for toggling
    jQuery(".toggle").on("click", ".toggle-title", function () {
        jQuery(this).toggleClass("active").closest('.toggle').find('.toggle-inner').slideToggle(200);
    });
});
