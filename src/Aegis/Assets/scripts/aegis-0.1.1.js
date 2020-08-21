(function () {
    if (window.aegis !== undefined) {
        return;
    }


    if (window.localStorage === undefined) {
        alert('Unsupported browser!');
    }

    else if (window.$ === undefined) {
        alert('AEGIS requires jQuery!');
    }

    else {
        var data = localStorage.getItem("x-aegis-kv");

        if (data !== undefined) {
            try {
                data = JSON.parse();
            } catch (e) {
                data = { seq: 0 };
            }
        }
        
        else data = { seq: 0 };

        function save() {
            localStorage.setItem('x-aegis-kv', JSON.stringify(data));
        }

        window.aegis = {
            authorize: function () {
                if (data.pvt === undefined) {
                    return this.register();
                }

                var request = { };

                if (aegis.csrf !== undefined) {
                    request.csrf = aegis.csrf;
                }

                /* request SECP256K1 key-pair. */
                $.ajax({
                    url: '/aegis/generate',
                    method: 'POST',
                    data: request
                }).done(function (res) {
                    if (res.pvt === undefined) {
                        try {
                            res = JSON.parse(res);
                        }

                        catch (e) {
                            alert('failed to generate SECP256K1 key-pair!');
                            return;
                        }
                    }


                }).fail(function (xhr) {

                });
            },

            register: function () {

            },

            unregister: function () {

            }
        };

        aegis.authorize();
    }
});