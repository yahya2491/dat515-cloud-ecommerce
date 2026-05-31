/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'standalone',
  // For Next.js 13+ App Directory, environment variables are automatically available
  // at runtime when prefixed with NEXT_PUBLIC_
  env: {
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:8080',
  },
};

export default nextConfig;
