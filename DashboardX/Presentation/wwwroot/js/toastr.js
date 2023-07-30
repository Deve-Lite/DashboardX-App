window.Toastr = (type, message) =>
{
    if (type === "success") {
        toastr.success(message,
            {
                timeOut: 1000
            });
    }
    if (type === "error") {
        toastr.error(message,
            {
                timeOut: 1000
            });
    }
    if (type === "info") {
        toastr.info(message,
            {
                timeOut: 1000
            });
    }
    if (type === "error") {
        toastr.warning(message,
            {
                timeOut: 1000
            });
    }
}