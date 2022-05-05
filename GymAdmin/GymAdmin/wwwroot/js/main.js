$(document).ready(function () {

    var slider = 3;
    function cambiarFondo() {
        slider--;
        if (slider == 0) {
            slider = 3;
        }
        if (slider == 1) {
            $("header").css("background-image", "url("+images[0].src+")");
        }
        else if (slider == 2) {
            $("header").css("background-image", "url(" + images[1].src + ")");
        }
        else if (slider == 3) {
            $("header").css("background-image", "url(" + images[2].src + ")");
        }
        $("h1").fadeOut(400);
        $(".titular").fadeOut(400);
        $(".bg-transition #img").delay(300).animate({
            left: "100%"
        }, 600, "swing", function () {
            var imagen = $("header").css("background-image");
            $(".bg-transition #img").css({
                left: "0%",
                backgroundImage: imagen,
                backgroundSize: "cover"
            });
            $("h1").delay(100).fadeIn(400);
            $(".titular").delay(100).fadeIn(400);
        });
    }

    function preloadImages(srcs, imgs, callback) {
        var img;
        var remaining = srcs.length;
        for (var i = 0; i < srcs.length; i++) {
            img = new Image();
            img.onload = function () {
                --remaining;
                if (remaining <= 0) {
                    callback();
                }
            };
            img.src = srcs[i];
            imgs.push(img);
        }
    }

    var imageSrcs = [
        "../../images/home/index/home-slider1.jpg",
        "../../images/home/index/home-slider2.jpg",
        "../../images/home/index/home-slider3.jpg"
    ];
    var images = [];

    preloadImages(imageSrcs, images, startfunction);

    var cambioDeFondo = 0;
    function startfunction() {
        cambioDeFondo = setInterval(cambiarFondo, 8000);
    }

    var tamañoPantalla = $(window).width();
    function comprobarTamañoPantalla() {
        if (tamañoPantalla <= 800) {
            clearInterval(cambioDeFondo);
            $(".bg-transition #img").hide();
        }
    }

    comprobarTamañoPantalla();
    $(window).resize(function () {
        if (tamañoPantalla != $(window).width()) {
            location.reload();
        }
    });

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

    var cerrar = false;
    $(".menu-icon").click(function () {
        $("nav").stop().slideToggle();
        if (!cerrar) {
            cerrar = true;
            $(this).css("transform", "rotate(90deg)");
        } else {
            cerrar = false;
            $(this).css("transform", "rotate(0deg)");
        }
    });
});
