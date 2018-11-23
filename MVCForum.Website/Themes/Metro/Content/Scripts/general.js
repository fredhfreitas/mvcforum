$(document).ready( function () {
    $('.collapse').on('shown.bs.collapse', function () {
        $(this).prev().children('i').removeClass("fa-chevron-down").addClass("fa-chevron-up");
    });
    
    $('.collapse').on('hidden.bs.collapse', function () {
        $(this).prev().children('i').removeClass("fa-chevron-up").addClass("fa-chevron-down");
    });
});