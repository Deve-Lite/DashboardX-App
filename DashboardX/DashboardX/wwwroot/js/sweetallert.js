window.ShowAllert = (type, message) => {
    if (type === "success") {
        Swal.fire({
            icon: 'success',
            title: message,
            showConfirmButton: false,
            timer: 1000
        })
    }
    if (type === "error") {
        Swal.fire({
            icon: 'errork',
            title: message,
            showConfirmButton: false,
            timer: 1000
        })
    }
}