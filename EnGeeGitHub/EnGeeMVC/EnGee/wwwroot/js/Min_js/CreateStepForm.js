function nextStep2()  //跳到第二頁1.第一頁的帳戶資料先存檔2.第二頁id=block,第一頁id=none
{
    var fullname = document.getElementById("fullname").value;
    var username = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    var repassword = document.getElementById("repassword").value;
    document.getElementById("stepform2").style.display = "block";
    document.getElementById("stepform1").style.display = "none";
}

function nextStep3() {//跳到第三頁1.第二頁的帳戶資料先存檔2.第三頁id=block,第二頁id=none
    var email = document.getElementById("email").value;
    var phone = document.getElementById("phone").value;
    var address = document.getElementById("address").value;
    var gender = document.getElementById("gender").value;
    var birth = document.getElementById("birth").value;
    document.getElementById("stepform3").style.display = "block";
    document.getElementById("stepform2").style.display = "none";
}
function back1() {
    document.getElementById("stepform1").style.display = "block";
    document.getElementById("stepform2").style.display = "none";
}
function nextStep4() {//跳到第四頁1.第三頁的帳戶資料先存檔2.第四頁id=block,第三頁id=none
    var photopath = document.getElementById("photopath").value;
    var introduction = document.getElementById("introduction").value;

    document.getElementById("stepform4").style.display = "block";
    document.getElementById("stepform3").style.display = "none";
}
function back2() {
    document.getElementById("stepform2").style.display = "block";
    document.getElementById("stepform3").style.display = "none";
}
function confirmSubmit() {
    document.getElementById("stepform2").style.display = "none";
    document.getElementById("stepform3").style.display = "block";

    var name = document.getElementById("name").value;
    var empid = document.getElementById("empid").value;
    var email = document.getElementById("email").value;
    var address = document.getElementById("address").value;
    var x = "Name:" + name + "<br>Employee Id:" + empid + "<br>Email:" + email + "<br>Address:" + address;
    document.getElementById("display").innerHTML = x;


}
function reject() {
    document.getElementById("stepform1").style.display = "block";
    document.getElementById("stepform2").style.display = "none";
}

function home() {
    document.getElementById("stepform").style.display = "block";
    document.getElementById("stepform3").style.display = "none";
    document.getElementById("name").value = '';
    document.getElementById("empid").value = '';
}