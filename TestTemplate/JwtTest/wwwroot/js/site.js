// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function updateTime() {
    var timeElement = document.getElementById("server-time");
    var response = await fetch("/api/values/gettime", {
        method: 'GET',
        credentials: "same-origin"
    });
    if (!response.redirected) {
        var text = await response.text();
        timeElement.innerText = "Время на сервере " + text;
        setTimeout(updateTime, 1000);
    }
    else {
        timeElement.innerText = "Время на сервере недоступно";
    }
}

setTimeout(updateTime, 1000);