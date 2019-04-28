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
var time = null;
var bar = $('#motus-info-bar');

function HideBar() {
    console.log("Hide");
    time = setTimeout(function () {
        bar.slideUp();
    }, 5000);
};


$('.player').on('mouseover', function (e) {
    bar.slideDown("fast");
    !time || clearTimeout(time);
    HideBar();
});

document.addEventListener('DOMContentLoaded', function () {
    function RenderPlayerInfoBar() {
        HideBar();
        Slide();
    }

    RenderPlayerInfoBar();
})