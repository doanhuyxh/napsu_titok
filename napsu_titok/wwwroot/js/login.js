document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('loginForm');
    const email = document.getElementById('email');
    const password = document.getElementById('password');
    const emailError = document.getElementById('emailError');
    const passwordError = document.getElementById('passwordError');
    const loginButton = document.getElementById('loginButton');
    const spinner = loginButton.querySelector('.spinner-border');
    const togglePassword = document.getElementById('togglePassword');

    const validateEmail = (email) => {
        const re = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return re.test(String(email).toLowerCase());
    };

    const validatePassword = (password) => {
        return password.length >= 8;
    };

    email.addEventListener('input', function () {
        if (!validateEmail(this.value)) {
            this.classList.add('is-invalid');
            emailError.textContent = 'Định dạng email không hợp lệ';
        } else {
            this.classList.remove('is-invalid');
            this.classList.add('is-valid');
            emailError.textContent = '';
        }
    });

    password.addEventListener('input', function () {
        if (!validatePassword(this.value)) {
            this.classList.add('is-invalid');
            passwordError.textContent = 'Mật khẩu phải có ít nhất 8 ký tự';
        } else {
            this.classList.remove('is-invalid');
            this.classList.add('is-valid');
            passwordError.textContent = '';
        }
    });

    togglePassword.addEventListener('click', function () {
        const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
        password.setAttribute('type', type);
        this.classList.toggle('bi-eye');
        this.classList.toggle('bi-eye-slash');
    });

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        if (validateEmail(email.value) && validatePassword(password.value)) {
            loginButton.disabled = true;
            spinner.classList.remove('d-none');

            let formData = new FormData()
            formData.append("email", $("#email").val())
            formData.append("password", $("#password").val())

            fetch("/login", {
                method: "POST",
                body: formData
            }).then(res => res.json())
                .then(data => {

                    loginButton.disabled = false;
                    spinner.classList.add('d-none');

                    if (data.code != 200) {
                        Swal.fire({
                            position: "center",
                            icon: "error",
                            title: "Tài khoản hoặc mật khẩu sai",
                            showConfirmButton: false,
                            timer: 3000
                        });
                    }

                    if (data.code == 200) {
                        Swal.fire({
                            position: "center",
                            icon: "success",
                            title: "Đăng nhập thành công, đang chuyển sang trang chủ",
                            showConfirmButton: false,
                            timer: 1500
                        }).then(res => {
                            window.location.href = data?.url || "/"
                        });
                    }
                })

        } else {
            email.classList.add('is-invalid');
            password.classList.add('is-invalid');
        }
    });
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

function login() {
    const email = document.getElementById("loginEmail").value.trim();
    const password = document.getElementById("loginPassword").value;
    const loginMsg = document.getElementById("loginMsg");

    let formData = new FormData()
    formData.append("email", email)
    formData.append("password", password)

    fetch("/login", {
        method: "POST",
        body: formData
    }).then(res => res.json())
        .then(data => {
            loginMsg.textContent = "Email hoặc mật khẩu không chính xác!";
            if (data.code === 200) {
                showModal(`Đăng nhập thành công! Xin chào, ${email}`);
                window.location.href = data?.url || "/"
            }
        })
        .catch(err => {
            loginMsg.textContent = "Email hoặc mật khẩu không chính xác!";
        })
}

