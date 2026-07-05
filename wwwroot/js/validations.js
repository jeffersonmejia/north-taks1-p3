document.querySelectorAll('input[type="number"]').forEach((input) => {
  input.addEventListener("input", () => {
    const value = Number(input.value);
    input.setCustomValidity(Number.isInteger(value) && value > 0 ? "" : "Enter a positive whole number.");
  });
});

document.querySelectorAll(".pwd-toggle").forEach((btn) => {
  btn.addEventListener("click", () => {
    const input = btn.parentElement.querySelector("input");
    const open = btn.querySelector(".eye-open");
    const closed = btn.querySelector(".eye-closed");
    if (!input) return;
    const show = input.type === "password";
    input.type = show ? "text" : "password";
    if (open) open.style.display = show ? "none" : "";
    if (closed) closed.style.display = show ? "" : "none";
  });
});
