const passwordInput = document.getElementById('passwordInput');
const eyeIcon = document.getElementById('eyeIcon');
const togglePassword = document.getElementById('togglePassword');

togglePassword.addEventListener('click', () => {
    const type = passwordInput.type === 'password' ? 'text' : 'password';
    passwordInput.type = type;

    // Change eye icon based on the password visibility state
    eyeIcon.className = type === 'password' ? 'bi bi-eye' : 'bi bi-eye-slash';
});