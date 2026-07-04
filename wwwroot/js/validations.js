document.querySelectorAll('input[type="number"]').forEach((input) => {
  input.addEventListener("input", () => {
    const value = Number(input.value);
    input.setCustomValidity(Number.isInteger(value) && value > 0 ? "" : "Enter a positive whole number.");
  });
});
