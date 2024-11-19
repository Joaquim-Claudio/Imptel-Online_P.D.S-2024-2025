const base_url = 'http://localhost:5293'

$(document).ready(function() {

    $('#loginForm').submit(async e => {
        e.preventDefault();

        var username = $('#usernameInput').val();
        var password = $('#passwordInput').val();

        if(!inputPatternCheck(username, password)){
            showPatternError();
            return
        } 


            Swal.fire({
                title: "Autenticação em curso...",
                text: "A verificar as tuas credenciais.",
                showConfirmButton: false,
                showLoaderOnConfirm: true,
                preConfirm: async () => {
                    Swal.showLoading();

                    return await fetch (base_url + '/api/accounts/login', {
                        headers: {'Content-Type': 'application/json'},
                        method: 'POST',
                        body: JSON.stringify({
                            username: username,
                            password: password
                        })
                    })
                    .then( response => {
                        if(!response.ok) {

                            showError();
                            Swal.close();

                            throw new Error(response.statusText);
                        }
                        
                        return response.json();
                        
                    })
                    .catch(error => {
                        Swal.fire({
                            title: "Upsss!",
                            text: "Algo correu mal... Tente outra vez dentro de alguns minutos.",
                            icon: "warning",
                            timer: 5000,
                            showConfirmButton: false,
                        })

                        return false;
                    })
        
                }, allowOutsideClick: () => !Swal.isLoading(),

                didOpen: () => {
                    Swal.clickConfirm();
                }
        
            }).then((result) => {

                console.log(result);

                if(result.value) {
                    if(!result.value.err_code) {

                        Swal.fire({
                            title:"Bem-vindo", 
                            icon: "success",
                            showConfirmButton: false,
                            timer: 3000,
                        });  
                            
                        setTimeout(() => {
                            window.location.href = "https://google.com";
                        }, 2000);
                        

                        
                    }
                }
            });


    })


    

    $(".form-control").on("change", function() {
        $(".form-control").css("border-color", "");
        $('#failMsg').hide();
        $('#ptrnMsg').hide();

    })

});

function inputPatternCheck (name, pass) {
    return !(name == "" || pass == "")
}

function showPatternError() {
    $('#usernameInput').css("border-color", "red");
    $('#passwordInput').css("border-color", "red");

    $('#ptrnMsg').show();
}

function showError() {
    $('#usernameInput').css("border-color", "red");
    $('#passwordInput').css("border-color", "red");

    $('#failMsg').show();
}