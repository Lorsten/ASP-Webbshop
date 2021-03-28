//* Event listener for the hamburgermenu
document.getElementById("Hambugermenu").addEventListener("click", function () {
    this.classList.toggle("is-active");
    let menuID = document.getElementById("Hambugermenu");

    let menu = document.querySelector(".navigation");
    menu.classList.toggle("active");
});

window.onload = () => {
    if (document.getElementById("cartbutton")) {
        document.getElementById("cartbutton").addEventListener("click", function () {
            let cart = document.querySelector(".cart");
            if (cart.classList.contains("active")) {
                cart.classList.remove("active");
            }
            else {
                cart.classList.toggle("active");
            }
        })
    } 
}
if (document.getElementById("close")) {
    document.getElementById("close").addEventListener("click", () => {
        let cart = document.querySelector(".cart");
        cart.classList.remove("active");
    })
}     


window.Alert = (message) => {
    alert(message);
}


window.BacktoHome = () => {
    window.location.href = '/';
}