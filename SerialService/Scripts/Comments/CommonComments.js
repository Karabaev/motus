cackle_widget = window.cackle_widget || [];
cackle_widget.push({
    widget: 'Comment', id: 65217, ssoAuth: '@ViewBag.UserToken', msg:
    {
        placeholder: 'Поделитесь своим мнением',
        submit: 'Оставить',
        sort: 'Порядок',
        social: "Авторизуйтесь на сайте"
    },
    stream: true, // автообновление комментов при появлении новых
    ssoPrimary: true, // приоритет авторизации на сайте
    // shareSocial: [] // соц. сети, куда можно расшарить
    callback: {
        //    create: [function (comment) { console.log(comment); }],
        //    edit: [function (comment) { console.log(comment); }],
        //    status: [function (comment) { console.log(comment); }],
        //    vote: [function (comment) { console.log(comment); }],
        //    submit: [function (comment) { console.log(comment); }],
        //    ready: [function (comment) { console.log(comment); }]
        vote: [function (comment) { alert("лайк коммента"); }]
    },
    channel: "films" + @Model.ID });
(function () {
    var mc = document.createElement('script');
    mc.type = 'text/javascript';
    mc.async = true;
    mc.src = ('https:' == document.location.protocol ? 'https' : 'http') + '://cackle.me/widget.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(mc, s.nextSibling);
})();