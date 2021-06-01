// fa-eye fa-eye-slash
const togglePassword = document.querySelector('#togglePassword');
const togglePassword2 = document.querySelector('#togglePassword2');
const password = document.querySelector('#Password');
const password2 = document.querySelector('#Password2');

function loginPassword() {
    const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
    password.setAttribute('type', type);
    // toggle the icon
    console.log(type);

    if (type == 'password') {
        $("#togglePassword").removeClass('fa-eye-slash');
        $("#togglePassword").addClass('fa-eye');
    } else {
        $("#togglePassword").addClass('fa-eye-slash');
    }
}

function loginPassword2() {
    const type = password2.getAttribute('type') === 'password' ? 'text' : 'password';
    password2.setAttribute('type', type);
    // toggle the icon
    console.log(type);

    if (type == 'password') {
        $("#togglePassword2").removeClass('fa-eye-slash');
        $("#togglePassword2").addClass('fa-eye');
    } else {
        $("#togglePassword2").addClass('fa-eye-slash');
    }
}