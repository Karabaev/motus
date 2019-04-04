function Slide() {
    const slider = document.getElementById('recommendations');
    let leftIsPress = false;
    let startX;
    let scrollLeft;
    slider.addEventListener('mousedown', (e) => {
        leftIsPress = true;
        slider.classList.add('active');
        startX = e.pageX - slider.offsetLeft;
        scrollLeft = slider.scrollLeft;
    })
    slider.addEventListener('mouseleave', () => {
        leftIsPress = false;
        slider.classList.remove('active');
    })
    slider.addEventListener('mouseup', () => {
        leftIsPress = false;
        slider.classList.remove('active');
    })
    slider.addEventListener('mousemove', (e) => {
        if (leftIsPress) {
            e.preventDefault();
            const x = e.pageX - slider.offsetLeft;
            const trac = (x - startX) * 3;
            slider.scrollLeft = scrollLeft - trac;
        }
    })
}

document.addEventListener('DOMContentLoaded', function () {
    function RenderPlayerInfoBar() {
        var bar = $('#motus-info-bar');
        var player = $('.player');
        var time = null;
        function HideBar() {
            time = setTimeout(function () {
                bar.slideUp();
            }, 1000);
        };

        HideBar();

        player.mousemove(function () {
            bar.slideDown("fast");
            !time || clearTimeout(time);
            HideBar()
        });

        Slide();
    }

    RenderPlayerInfoBar();
})