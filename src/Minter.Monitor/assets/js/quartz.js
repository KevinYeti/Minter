/*第一次读取最新通知*/
setTimeout(function () { Push(); }, 200);

/*20轮询读取函数*/
//setInterval(function () { Push(); }, 20000);

/*请求函数的ajax*/
function Push() {
    $.ajax({
        type: "GET",
        url: "/Api/Scheduler",
        data: {
            t: 3
        },
        beforeSend: function () { },
        success: function (data) {
            app.sched = data; 
        }
    });
}

var sched = null;

var app = new Vue({
    el: '#schedulerContainer',
    data: {
        sched: ""
    }
})