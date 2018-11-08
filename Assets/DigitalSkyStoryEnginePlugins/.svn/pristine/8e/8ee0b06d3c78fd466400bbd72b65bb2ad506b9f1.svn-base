using System.Threading;

/******************************************************************************
* [2010] - [2017] Dynamixyz
* All Rights Reserved.
*
* NOTICE:  All information contained herein is, and remains the property
* of Dynamixyz and its suppliers,if any. The intellectual and technical
* concepts contained herein are proprietary to Dynamixyz and its suppliers
* and may be covered by U.S. and Foreign Patents, patents in process, and
* are protected by trade secret or copyright law.
* Dissemination of this information or reproduction of this material
* is strictly forbidden unless prior written permission is obtained
* from Dynamixyz.
*
******************************************************************************/

/* Author: Louis-Paul CORDIER - lp.cordier@dynamixyz.com */

/* CHANGELOG:
 * v 1.1:
 * + Fix deadlock in xxxRelease() methods.
 * 
 * v 1.0: initial release.
 *  */

namespace dxyz
{
    public class PingPongBuffer
    {
        protected byte[] mPing;
        protected Mutex mMtx_ping;

        protected byte[] mPong;
        protected Mutex mMtx_pong;
        protected Semaphore mAvailableData;

        protected STATUS mLastWrittenBuf;
        protected STATUS mReadBuf;

        protected enum STATUS
        {
            PINGPONG_PING = 1,
            PINGPONG_PONG = 2
        }

        public PingPongBuffer(ulong iBufSz)
        {
            mMtx_ping = new Mutex();
            mMtx_pong = new Mutex();
            mAvailableData = new Semaphore(0, 1);

            mPing = new byte[iBufSz];
            mPong = new byte[iBufSz];
        }

        ~PingPongBuffer()
        {
            mMtx_ping = null;
            mMtx_pong = null;
            mAvailableData = null;

            mPing = null;
            mPong = null;
        }

        public void writeRequest(out byte[] oBuf)
        {
            lock (this)
            {
                if (mLastWrittenBuf == STATUS.PINGPONG_PING)
                {
                    mMtx_pong.WaitOne();
                    oBuf = mPong;
                }
                else // PINGPONG_PONG
                {
                    mMtx_ping.WaitOne();
                    oBuf = mPing;
                }
            }
        }

        public void writeRelease()
        {
            if (mLastWrittenBuf == STATUS.PINGPONG_PING)
            {
                mLastWrittenBuf = STATUS.PINGPONG_PONG;
                mMtx_pong.ReleaseMutex();
            }
            else // PINGPONG_PONG
            {
                mLastWrittenBuf = STATUS.PINGPONG_PING;
                mMtx_ping.ReleaseMutex();
            }

            mAvailableData.WaitOne(0);
            mAvailableData.Release(1);
        }

        public void writeCancelRequest()
        {
            if (mLastWrittenBuf == STATUS.PINGPONG_PING)
            {
                mMtx_pong.ReleaseMutex();
            }
            else // PINGPONG_PONG
            {
                mMtx_ping.ReleaseMutex();
            }
        }

        public bool readRequest(out byte[] oBuf)
        {
            lock (this)
            {
                if (!mAvailableData.WaitOne(0))
                {
                    oBuf = null;
                    return false;
                }

                if (mLastWrittenBuf == STATUS.PINGPONG_PING)
                {
                    mMtx_ping.WaitOne();
                    mReadBuf = STATUS.PINGPONG_PING;
                    oBuf = mPing;
                }
                else // PINGPONG_PONG
                {
                    mMtx_pong.WaitOne();
                    mReadBuf = STATUS.PINGPONG_PONG;
                    oBuf = mPong;
                }
            }

            return true;
        }

        public void readRelease()
        {
            if (mReadBuf == STATUS.PINGPONG_PING)
            {
                mMtx_ping.ReleaseMutex();
            }
            else // PINGPONG_PONG
            {
                mMtx_pong.ReleaseMutex();
            }
        }
    }
}