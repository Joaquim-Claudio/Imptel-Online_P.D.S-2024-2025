
const base_url = 'http://localhost:5293'

$(document).ready(function() {

    $('#loginForm').submit(e => {
        e.preventDefault();

        var username = $('#usernameInput').val();
        var password = $('#passwordInput').val();

        fetch (base_url + '/api/accounts/login', {
            headers: {'Content-Type': 'application/x-www-form-urlencoded'},
            method: 'POST',
            body: {
                "username": username,
                "password": password
            }

        }).then(response => {
            if(!response.ok) {
                throw new Error(response.statusText);
            }

            console.log(response.json());
        })


    })

});