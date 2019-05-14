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
    time = setTimeout(function () {
        bar.slideUp();
    }, 3000);
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

var counter = 0;

function onPlayerTimeUpdate(player_time) {
    //console.log('onPlayerTimeUpdate', player_time);

    counter++;

    if (counter >= 5) {
        counter = 0;
        model = {
            TimeSec: Math.round(player_time),
            SeasonNumber: null,
            EpisodeNumber: null,
            TranslatorName: "Многоголосый закадровый",
            VideoMaterialID: $("#VideoMaterialIDHdn").val()
        };

        $.ajax({
            method: 'post',
            data: model,
            url: '/User/SaveViewTime',
            success: function (data) {
                if (data.success) {
                    console.log(data.success);
                }
                else {
                    if (data.error) {
                        console.error(data.error);
                    }
                }
            }
        });
    }
};

function mwPlayerMessageReceive(event) {
    if (event.data && event.data.message == 'MW_PLAYER_TIME_UPDATE') {
        onPlayerTimeUpdate(event.data.value);
    }
};

$(function () {
    if (window.addEventListener) {
        window.addEventListener('message', mwPlayerMessageReceive);
    } else {
        window.attachEvent('onmessage', mwPlayerMessageReceive);
    }
});