$(document).ready(function () {
    $('.photo').hover(
        function () {
            $(this).find('.dark-backdrop-hidden').stop().fadeIn();
            $(this).find('.img-hover').addClass('dark-backdrop-showed');
        },
        function () {
            $(this).find('.dark-backdrop-hidden').stop().fadeOut();
            $(this).find('.img-hover').removeClass('dark-backdrop-showed');
        }
    );
});
