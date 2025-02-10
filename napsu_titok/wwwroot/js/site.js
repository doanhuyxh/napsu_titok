document.addEventListener("DOMContentLoaded", function () {
    // Đăng xuất với xác nhận
    const logoutBtn = document.getElementById("logout");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", function () {
           $("#logoutModal").modal("show");
        });
    }

    // Sidebar Toggle
    const menuToggle = document.getElementById("menu-toggle");
    const sidebar = document.getElementById("sidebar");
    const closeSidebar = document.getElementById("close-sidebar");

    if (menuToggle && sidebar && closeSidebar) {
        menuToggle.addEventListener("click", function (event) {
            event.stopPropagation();
            sidebar.classList.toggle("active");
        });

        closeSidebar.addEventListener("click", function () {
            sidebar.classList.remove("active");
        });

        document.addEventListener("click", function (event) {
            if (!sidebar.contains(event.target) && !menuToggle.contains(event.target) && sidebar.classList.contains("active")) {
                sidebar.classList.remove("active");
            }
        });
    }

    // Avatar Dropdown Toggle
    const avatar = document.getElementById("avatar");
    const profileMenu = document.getElementById("profile-menu");

    if (avatar && profileMenu) {
        avatar.addEventListener("click", function (event) {
            event.stopPropagation();
            profileMenu.classList.toggle("visible");
        });

        document.addEventListener("click", function (event) {
            if (!profileMenu.contains(event.target) && event.target !== avatar) {
                profileMenu.classList.remove("visible");
            }
        });
    }
});
