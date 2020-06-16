function Noop(data) {
    console.log("TODO: placeholder was called")
    console.log(data);
}

function AjaxLoad(url, responseType, onSuccess, onFailure) {
    let xhttp = new XMLHttpRequest();
    xhttp.responseType = responseType;
    xhttp.onreadystatechange = function () {
        if (this.readyState != 4)
            return;

        if (this.status == 200) {
            onSuccess(xhttp.response);
        } else {
            onFailure(xhttp.response);
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

/**
 * Issue a POST request with JSON content
 * @param {string} url 
 * @param {object} data Will turn this to JSON string
 * @param {function} onSuccess 
 * @param {function} onFailure 
 */
function AjaxPost(url, data, onSuccess, onFailure) {
    let xhttp = new XMLHttpRequest();
    xhttp.responseType = "json";
    xhttp.onreadystatechange = function () {
        if (this.readyState != 4)
            return;

        if (this.status == 200) {
            onSuccess(xhttp.response);
        } else {
            onFailure(xhttp.response);
        }
    };
    xhttp.open("POST", url);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhttp.send(JSON.stringify(data));
}

function DownloadTemplate(filename, loadAction, failAction) {
    AjaxLoad(filename, "text", loadAction, failAction);
}

/**
 * Will call loadAction with all the loaded templates in a hashmap
 * - loads all templates
 * - collects them in a hashmap
 * - calls loadAction when all of them have been loaded, passing the hashmap
 * @param {string[]} templateFiles 
 * @param {function(contents)} loadAction 
 */
function LoadTemplates(templateFiles, loadAction, failAction) {
    let contents = {};
    let toLoadCount = templateFiles.length;
    templateFiles.forEach(function (fn) {
        let filename = fn;
        DownloadTemplate(filename, function (content) {
            contents[filename] = content;
            console.log("stored " + filename)
            toLoadCount -= 1;

            if (toLoadCount == 0)
                loadAction(contents);
        }, function(error) {
            console.log(error);
            failAction(filename);
        });
    });
}


export { Noop, AjaxLoad, AjaxPost, LoadTemplates };