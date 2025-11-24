/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        "primary-orange": "#FF6B35",
        "primary-dark": "#2C3E50",
        "primary-blue": "#3498DB",
      },
    },
  },
  plugins: [],
};
