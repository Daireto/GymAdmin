$(document).ready(function () {

    $("#Unete").delay(3500).fadeIn(400);
    $(".titular").delay(3000).fadeIn(400);

    //Gallery scale effect
    $('.photo').hover(
        function () {
            $(this).find('.oculto').stop().fadeIn();
            $(this).find('.img-hover').addClass('agrandar');
        },
        function () {
            $(this).find('.oculto').stop().fadeOut();
            $(this).find('.img-hover').removeClass('agrandar');
        }
    );
});
