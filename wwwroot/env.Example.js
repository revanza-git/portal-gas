let env = "local"; //local|dev|preprod|prod
function getBaseUrl() {
    if (env == "dev") {
        return "http://your.dev.url";
    } else if (env == "preprod") {
        return "http://your.preprod.url";
    } else if (env == "prod") {
        return "https://your.prod.url";
    } else {
        //return "http://localhost:14417";
        return "http://your.local.url";
    }
}

function getOvertimeFormUrl() {
    return "https://your.overtime.form.url"
}
