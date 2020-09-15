// Plugin @RokoCB :: Return the visible amount of px
// of any element currently in viewport.
// stackoverflow.com/questions/24768795/
; (function ($, win) {
    $.fn.inViewport = function (cb) {
        return this.each(function (i, el) {
            function visPx() {
                var H = $(this).height(),
                    r = el.getBoundingClientRect(), t = r.top, b = r.bottom;
                return cb.call(el, Math.max(0, t > 0 ? H - t : (b < H ? b : H)));
            } visPx();
            $(win).on("resize scroll", visPx);
        });
    };
}(jQuery, window));

$(function () {
    
    var vars = [], hash;
    var q = document.URL.split('?')[1];
    if (q !== undefined) {
        q = q.split('&');
        for (var i = 0; i < q.length; i++) {
            hash = q[i].split('=');
            vars.push(hash[1]);
            vars[hash[0]] = hash[1];
        }
    }
    if (vars['__r'] !== undefined) {
        ga('send', 'event', 'Leads', 'ContactUs');
    }
});

$(".strainEffectsBox").inViewport(function (px) {
    if (px > 125) {
        $(".progress-element").each(function () {
            var progressBar = $(".progress-bar");
            progressBar.each(function (indx) {
                $(this).css("width", $(this).attr("aria-valuenow") + "%");
            });
        });
    }
});

$('.nav-tabs a').on('show.bs.tab', function (event) {
    $(".progress-element").each(function () {
        var progressBar = $(".progress-bar");
        progressBar.each(function (indx) {
            $(this).css("width", "0%");
        });
    });
});

$('.nav-tabs a').on('shown.bs.tab', function (event) {
    $(".progress-element").each(function () {
        var progressBar = $(".progress-bar");
        progressBar.each(function (indx) {
            $(this).css("width", $(this).attr("aria-valuenow") + "%");
        });
    });
});

$("#Nav a").click(function (e) {
    e.preventDefault();
});

