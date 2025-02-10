$("#sidebarToggle, #sidebarToggleTop").on('click', function (e) {
    $("body").toggleClass("sidebar-toggled");
    $(".sidebar").toggleClass("toggled");
    if ($(".sidebar").hasClass("toggled")) {
        $('.sidebar .collapse').collapse('hide');
    };
});

function alertKOC() {
    let timerInterval;
    Swal.fire({
        title: "Thống báo tình trạng!",
        html: "Tình trạng có thể để được phát triển theo yêu cầu",
        timer: 9000,
        timerProgressBar: true,
        showCloseButton: true,
        showCancelButton: true,
        didOpen: () => {
            Swal.showLoading();
            const timerElement = Swal.getHtmlContainer().querySelector("b");
            timerInterval = setInterval(() => {
                timerElement.textContent = Swal.getTimerLeft();
            }, 100);
        },
        willClose: () => {
            clearInterval(timerInterval);
        }
    }).then((result) => {
        if (result.dismiss === Swal.DismissReason.timer) {
            // Hành động khi hộp thoại tự động đóng
        }
    });
}

const currentUrl = window.location.href;

function getMainDomainFromUrl(url) {
    const parsedUrl = new URL(url);
    const host = parsedUrl.host;
    if (host.startsWith("www.")) {
        return host.substring(4);
    }
    return host;
}

const mainDomain = getMainDomainFromUrl(currentUrl);
const currentYear = new Date().getFullYear();

const copyrightText = document.getElementById("copyright");
if (copyrightText) {
    copyrightText.textContent = `Copyright © ${currentYear} ${mainDomain}`;
}



function LoadModal(url, title) {
    $("#staticBackdropContent").load(url, function () {
        $("#staticBackdropLabel").html(title)
        $("#staticBackdrop").modal("show")
    })
}

function HideModal() {
    $("#staticBackdrop").modal("hide")
}

function showError(input, message) {
    const error = document.createElement('span');
    error.className = 'error-message text-danger';
    error.innerText = message;
    input.insertAdjacentElement('afterend', error);
}

function ShowNotify(icon, title) {
    return Swal.fire({
        title: title,
        icon: icon,
        timer: 2000,
        showConfirmButton: false
    })
}

const Toast = Swal.mixin({
    toast: true,
    position: "top-end",
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.onmouseenter = Swal.stopTimer;
        toast.onmouseleave = Swal.resumeTimer;
    }
});

function convertImageToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();

        reader.onload = function (e) {
            resolve(e.target.result);
        };

        reader.onerror = function (error) {
            console.log("Lỗi chuyển đổi sang base64")
            console.log(error)
            resolve("");
        };
        reader.readAsDataURL(file);
    });
}
function ViewImage(url) {
    Swal.fire({
        imageUrl: url,
        imageWidth: 400,
        imageHeight: 200,
    });
}