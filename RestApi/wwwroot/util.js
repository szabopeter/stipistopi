function AjaxLoad(url, responseType, onSuccess, onFailure) {
    let xhttp = new XMLHttpRequest();
    xhttp.responseType = responseType;
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            onSuccess(xhttp.response);
        } else {
            onFailure();
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

export { AjaxLoad };