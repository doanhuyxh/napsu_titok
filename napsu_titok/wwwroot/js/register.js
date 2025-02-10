const form = document.getElementById('signupForm');
const emailInput = document.getElementById('email');
const passwordInput = document.getElementById('password');
const submitButton = form.querySelector('button[type="submit"]');
const spinner = submitButton.querySelector('.spinner-border');

function showError(input, message) {
    input.classList.add('is-invalid');
    const errorDiv = input.nextElementSibling;
    errorDiv.textContent = message;
}

function clearError(input) {
    input.classList.remove('is-invalid');
    const errorDiv = input.nextElementSibling;
    errorDiv.textContent = '';
}

function validateEmail() {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(emailInput.value)) {
        showError(emailInput, 'Invalid email format');
        return false;
    }
    clearError(emailInput);
    return true;
}

function validatePassword() {
    if (passwordInput.value.length < 8) {
        showError(passwordInput, 'Password must be at least 8 characters long');
        return false;
    }
    clearError(passwordInput);
    return true;
}

emailInput.addEventListener('blur', validateEmail);
passwordInput.addEventListener('blur', validatePassword);

form.addEventListener('submit', function (event) {
    event.preventDefault();
    if (validateEmail() && validatePassword()) {
        spinner.classList.remove('d-none');
        submitButton.disabled = true;

        let hostName = window.location.hostname

        let formData = new FormData()

        formData.append("email", $("#email").val())
        formData.append("password", $("#password").val())
        formData.append("hostName", hostName)

        fetch("/register", {
            method: "POST",
            body: formData
        })
            .then(res => res.json())
            .then(data => {
                spinner.classList.add('d-none');
                submitButton.disabled = false;
                form.reset();

                if (data.code != 200) {
                    Swal.fire({
                        position: "center",
                        icon: "error",
                        title: "Email đã tồn tại vui lòng dùng email khác",
                        showConfirmButton: false,
                        timer: 3000
                    });
                }

                if (data.code == 200) {
                    Swal.fire({
                        position: "center",
                        icon: "success",
                        title: "Đăng ký thành công chuyển sang màn hình đăng nhập",
                        showConfirmButton: false,
                        timer: 1500
                    }).then(res => {
                        window.location.href = "/"
                    });
                }


            })

    }
});

const emailSuggestions = document.getElementById('emailSuggestions');
emailInput.addEventListener('input', function () {
    const atIndex = this.value.indexOf('@');
    if (atIndex !== -1) {
        const domain = this.value.slice(atIndex);
        const matchingOption = Array.from(emailSuggestions.options).find(option => option.value.startsWith(domain));
        if (matchingOption) {
            this.value = this.value.slice(0, atIndex) + matchingOption.value;
        }
    }
});


document.addEventListener('DOMContentLoaded', function () {
    const urlParams = new URLSearchParams(window.location.search);
    const refCode = urlParams.get('ref')
    console.log(refCode);
    localStorage.setItem("refCode", refCode);
});


function showModal(message) {
    const modal = document.getElementById("successModal");
    const modalMessage = document.getElementById("modalMessage");
    modalMessage.textContent = message;  // Gán nội dung
    modal.style.display = "block";       // Hiển thị modal
}

// Đóng modal khi nhấn dấu x
document.getElementById("closeModal").addEventListener("click", () => {
    document.getElementById("successModal").style.display = "none";
});

// Nếu muốn đóng modal khi click ra ngoài modal-content:
window.addEventListener("click", (event) => {
    const modal = document.getElementById("successModal");
    if (event.target === modal) {
        modal.style.display = "none";
    }
});


function showSignupForm() {
    document.getElementById("signupForm").classList.remove("hidden");
    document.getElementById("loginForm").classList.add("hidden");

    // Xoá thông báo cũ
    document.getElementById("signupMsg").textContent = "";
    document.getElementById("loginMsg").textContent = "";
}

// Hàm đăng ký
function signup() {
    //const name = document.getElementById("signupName").value.trim();
    const email = document.getElementById("signupEmail").value.trim();
    const password = document.getElementById("signupPassword").value;
    const confirmPassword = document.getElementById("signupConfirmPassword").value;
    const signupMsg = document.getElementById("signupMsg");

    // Kiểm tra nhập liệu cơ bản
    if (!email || !password || !confirmPassword) {
        signupMsg.textContent = "Vui lòng điền đầy đủ thông tin!";
        return;
    }

    // Kiểm tra mật khẩu có khớp không
    if (password !== confirmPassword) {
        signupMsg.textContent = "Mật khẩu nhập lại không khớp!";
        return;
    }

    let formData = new FormData()
    formData.append("email", email)
    formData.append("password", password)
    formData.append("fullname", "")
    formData.append("ref_code", localStorage.getItem("refCode"))

    fetch("/register", {
        method: "POST",
        body: formData
    })
        .then(res => res.json())
        .then(data => {

            if (data.code != 200) {
                signupMsg.textContent = "Email đã tồn tại!";
            }

            if (data.code == 200) {
                showModal("Đăng ký thành công! Bạn có thể đăng nhập.");
                window.location.href = "/"
            }


        })
        .catch(err => {
            signupMsg.textContent = "Đăng ký thất bại!";
        })

}